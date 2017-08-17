// Copyright © 2008 David S. Bakin
// This work licensed under The Code Project Open License (CPOL) 1.02

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

/*
namespace UISleuth.Reflection
{
    /// <summary>
    /// A class which lets you hook any or all events raised by an particular
    /// object.
    /// </summary>
    /// <remarks>
    /// To use, create a Tracer instance supplying a target object and a delegate
    /// which will receive the hooked events (the delegate is of type OnEventHandler,
    /// <see cref="OnEventHandler"/>).  Then hook the events you want to trace,
    /// by name.  (Get the names via the property <see cref="EventNames"/>.)
    /// </remarks>
    internal sealed class Tracer : IDisposable
    {
        /// <summary>
        /// Specifies the event handler that the user creates to hook events from the 
        /// target object.
        /// </summary>
        /// <param name="sender">Object raising the event</param>
        /// <param name="target">Object that this tracer is hooking</param>
        /// <param name="eventName">Name of the event the target raised</param>
        /// <param name="e">The EventArgs of the event the target raised</param>
        /// <remarks>
        /// Sender can be different than target if the event is a static event.
        /// When a tracer hooks a target's class' static event the
        /// OnEventHandler gets called every time that static event is raised,
        /// no matter which instance is raising it.
        /// </remarks>
        public delegate void OnEventHandler( object sender, 
                                             object target, 
                                             string eventName, 
                                             EventArgs e );

        /// <summary>
        /// Constructs an event Tracer.
        /// </summary>
        /// <param name="target">The object whose events are to be hooked</param>
        /// <param name="handler">User-supplied method gets called whenever the 
        /// target raises a hooked event</param>
        public Tracer( object target, OnEventHandler handler )
        {
            Debug.Assert( target != null );
            m_type = target.GetType( );
            m_target = target;
            m_handler = handler;
        }

        /// <summary>
        /// Unhooks all events when Tracer is Disposed.
        /// </summary>
        public void Dispose()
        {
            if ( EventsHookedCount > 0 )
            {
                UnhookAllEvents( );
            }
            m_target = null;
            m_handler = null;
        }

        #region Target of this Tracer
        private OnEventHandler m_handler;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private readonly Type m_type;
        /// <summary>
        /// The target's type
        /// </summary>
        public Type TheType
        {
            [DebuggerStepThrough]
            get 
            {
                return m_type; 
            }
       }

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private object m_target;
        /// <summary>
        /// The target object of this Tracer
        /// </summary>
        public object TheTarget
        {
            [DebuggerStepThrough]
            get
            {
                return m_target;
            }
        }
        #endregion

        #region Information on the target classes' events
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private EventInfo[] events = null;
        /// <summary>
        /// Information on all public events that can be raised by the target
        /// </summary>
        private EventInfo[] Events
        {
            get
            {
                if ( events == null )
                {
                    events = m_type.GetEvents( System.Reflection.BindingFlags.Instance |
                                               System.Reflection.BindingFlags.Static |
                                               System.Reflection.BindingFlags.Public |
                                               System.Reflection.BindingFlags.FlattenHierarchy );

                    // Now process events to strip out the ones which do not
                    // match the .NET Framework design pattern for events.
                    List<EventInfo> okEvents = new List<EventInfo>( );
                    List<string> okEventNames = new List<string>( );
                    List<string> notOkEventNames = new List<string>( );

                    foreach ( EventInfo eventInfo in Events )
                    {
                        if ( IsGoodNetFrameworkEvent( eventInfo ) )
                        {
                            okEvents.Add( eventInfo );
                            okEventNames.Add( eventInfo.Name );
                        }
                        else
                        {
                            notOkEventNames.Add( eventInfo.Name );
                        }
                    }

                    okEventNames.Sort( );
                    notOkEventNames.Sort( );

                    events = okEvents.ToArray( );
                    eventNames = okEventNames.ToArray( );
                    untraceableEventNames = notOkEventNames.ToArray( );
                }

                return events;
            }
        }

        /// <summary>
        /// A Set - if the key is present then that EventInfo is a static
        /// event
        /// </summary>
        private Dictionary<EventInfo, object> isStaticEvent = null;

        /// <summary>
        /// Predicate returns true iff the given event is a static event of 
        /// the target's class
        /// </summary>
        private bool IsStaticEvent( EventInfo eventInfo )
        {
            if ( isStaticEvent == null )
            {
                // This is odd:  Using Type.FindMembers with
                // MemberTypes.Event and BindingFlags.Static returns ALL
                // events, instance and static, where Type.GetEvents with
                // BindingFlags.Static will only return static events.
                EventInfo[] staticEvents = 
                    m_type.GetEvents( System.Reflection.BindingFlags.Static |
                                      System.Reflection.BindingFlags.Public |
                                      System.Reflection.BindingFlags.FlattenHierarchy );

                isStaticEvent = new Dictionary<EventInfo, object>( );
                foreach ( EventInfo eventInfo2 in staticEvents )
                {
                    isStaticEvent[ eventInfo2 ] = null;
                }
            }
            return isStaticEvent.ContainsKey( eventInfo );
        }

        /// <summary>
        /// Predicate returns true iff the given string is the name of a 
        /// static event of the target's class
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool IsStaticEvent( string eventName )
        {
            if ( !IsValidEvent( eventName ) )
            {
                throw new ArgumentException( "No event with that name" );
            }
            return IsStaticEvent( Array.Find( Events, EventHasName( eventName ) ) );
        }

        /// <summary>
        /// Determine if the event's signature matches the .NET Framework
        /// design pattern for events:  A Method with two parameters, first
        /// parameter is of type System.Object, and second parameter is of
        /// type System.EventArgs or some class derived from System>.EventArgs
        /// (which includes System.EventHandler{TEventArgs}), and the return type
        /// must be void.
        /// </summary>
        /// <param name="eventInfo"></param>
        private bool IsGoodNetFrameworkEvent( EventInfo eventInfo )
        {
            // First, from the EventInfo get the delegate type.
            Type eventHandlerType = eventInfo.EventHandlerType;
            Debug.Assert( eventHandlerType.BaseType == typeof( System.MulticastDelegate ) );
            if ( eventHandlerType.BaseType != typeof( System.MulticastDelegate ) )
            {
                // Must not have been an event after all?!?
                return false;
            }

            // The signature we want to look at is the delegate's Invoke method.
            MethodInfo invoke = eventHandlerType.GetMethod( "Invoke" );
            Debug.Assert( invoke != null );
            if ( invoke == null )
            {
                // Must not have been a delegate after all?!?
                return false;
            }

            // Get the delegate's parameter list.
            ParameterInfo[] parameters = invoke.GetParameters( );

            // Must have exactly 2 parameters
            if ( parameters.Length != 2 )
            {
                return false;
            }

            // First parameter must have type System.Object
            Type param1 = parameters[0].ParameterType;
            if ( param1 != typeof( System.Object ) )
            {
                return false;
            }
            
            // Second parameter must be System.EventArgs, or something derived
            // from System.EventArgs.
            Type param2 = parameters[1].ParameterType;
            if ( param2 != typeof( System.EventArgs ) &&
                 !param2.IsSubclassOf( typeof( System.EventArgs ) ) )
            {
                return false;
            }

            // Return type must be void
            if ( invoke.ReturnType != typeof( void ) )
            {
                return false;
            }

            // All tests pass - this is a standard .NET Framework event type
            return true;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string[] eventNames = null;
        private string[] EventNamesInternal
        {
            get
            {
                if ( eventNames == null )
                {
                    EventInfo[] events = Events;
                    Debug.Assert( eventNames != null );
                }
                return eventNames;
            }
        }

        /// <summary>
        /// A list of the names of all the events that can be raised by the target
        /// </summary>
        public ReadOnlyCollection<string> EventNames
        {
            get
            {
                return new ReadOnlyCollection<string>( EventNamesInternal );
            }
        }

        /// <summary>
        /// Predicate returns true iff the given string is the name of an event
        /// the target can raise
        /// </summary>
        public bool IsValidEvent( string eventName )
        {
            return Array.BinarySearch( EventNamesInternal, eventName ) >= 0;
            // (Relies on the EventNames being sorted.)
        }

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string[] untraceableEventNames = null;
        /// <summary>
        /// A list of the names of all the events that can be raised by the
        /// target but that the Tracer can't trace.  These events are those
        /// which don't follow the .NET Framework design pattern for events.
        /// </summary>
        public ReadOnlyCollection<string> UntraceableEventNames
        {
            get
            {
                if ( untraceableEventNames == null )
                {
                    EventInfo[] events = Events;
                    Debug.Assert( untraceableEventNames != null );
                }
                return new ReadOnlyCollection<string>( untraceableEventNames );
            }
        }
        #endregion

        #region Keeping track of hooked events
        /// <summary>
        /// Dictionary maps event names to the delegate subscribing to the event
        /// </summary>
        private Dictionary<string, Delegate> m_hooks = 
            new Dictionary<string, Delegate>( );

        /// <summary>
        /// Returns number of events currently hooked
        /// </summary>
        public int EventsHookedCount
        {
            [DebuggerStepThrough]
            get
            {
                return m_hooks.Count;
            }
        }

        /// <summary>
        /// Predicate returns true iff event (specified by its name) is 
        /// currently hooked
        /// </summary>
        public bool IsHookedEvent( string eventName )
        {
            return m_hooks.ContainsKey( eventName );
        }
        #endregion

        #region Class EventProxy for thunking events with their string name
        /// <summary>
        /// An instance of this class is created for every event that is hooked. 
        /// It's purpose is to hold the event's name so that the user can use 
        /// only one method to handle all of the events being hooked on a given
        /// target object.
        /// </summary>
        private sealed class EventProxy
        {
            private readonly OnEventHandler m_handler;
            private readonly object m_target;
            private readonly string m_name;

            public EventProxy( Tracer parent, object target, string name )
            {
                m_handler = parent.m_handler;
                m_target = target;
                m_name = name;
            }

            public void OnEvent( object sender, EventArgs e )
            {
                if ( m_handler != null )
                {
                    m_handler.Invoke( sender, m_target, m_name, e );
                }
            }

            /// <summary>
            /// The MethodInfo of the event raised in this class (a cache 
            /// so Reflection need be used only once)
            /// </summary>
            public static MethodInfo OnEventMethodInfo =
                typeof( EventProxy ).GetMethod( "OnEvent" );
        }
        #endregion

        #region Hooking and Unhooking Events
        /// <summary>
        /// Given an EventInfo for an event of the target instance's class, hook it
        /// </summary>
        private void HookEvent( EventInfo eventInfo )
        {
            // Each event is only hooked once
            if ( TheTarget != null && !IsHookedEvent( eventInfo.Name ) )
            {
                // To hook an event: Create an EventProxy for it, then create
                // a delegate to the EventProxy's OnEvent method, then add the
                // delegate to the event's list
                EventProxy proxy = new EventProxy( this, TheTarget, eventInfo.Name );
                Delegate d = Delegate.CreateDelegate( eventInfo.EventHandlerType, 
                                                      proxy, 
                                                      EventProxy.OnEventMethodInfo );
                m_hooks[ eventInfo.Name ] = d;
                eventInfo.AddEventHandler( TheTarget, d );
            }
        }

        /// <summary>
        /// Hook an event of the target instance's class, given its name
        /// </summary>
        public void HookEvent( string eventName )
        {
            if ( !IsValidEvent( eventName ) )
            {
                throw new ArgumentException( "No event with that name" );
            }
            HookEvent( Array.Find( Events, EventHasName( eventName ) ) );
        }

        /// <summary>
        /// Hook all events that the instance can raise
        /// </summary>
        public void HookAllEvents( )
        {
            foreach ( EventInfo eventInfo in Events )
            {
                HookEvent( eventInfo );
            }
        }

        /// <summary>
        /// Given an EventInfo for an event of the target instance's class, unhook it
        /// </summary>
        private void UnhookEvent( EventInfo eventInfo )
        {
            if ( TheTarget != null && IsHookedEvent( eventInfo.Name ) )
            {
                eventInfo.RemoveEventHandler( TheTarget, m_hooks[ eventInfo.Name ] );
                m_hooks.Remove( eventInfo.Name );
            }
        }

        /// <summary>
        /// Unhook an event of the target instance's class, given its name
        /// </summary>
        public void UnhookEvent( string eventName )
        {
            if ( !IsValidEvent( eventName ) )
            {
                throw new ArgumentException( "No event with that name" );
            }
            UnhookEvent( Array.Find( Events, EventHasName( eventName ) ) );
        }

        /// <summary>
        /// Unhook all events that have been hooked
        /// </summary>
        public void UnhookAllEvents()
        {
            // Make a copy of m_hooks.Keys because it is going to get
            // modified during the foreach - m_hooks.Keys does not
            // return a copy of the Keys of a Dictionary, it returns the
            // actual KeysCollection.
            foreach ( string name in new List<string>( m_hooks.Keys ) )
            {
                UnhookEvent( name );
            }
        }

        /// <summary>
        /// Factory that returns a predicate that returns true if the EventInfo
        /// it is passed is for the event whose name is passed to the factory
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private static Predicate<EventInfo> EventHasName( string eventName )
        {
            return delegate( EventInfo eventInfo ) 
                   {
                       return eventInfo.Name == eventName;
                   };
        }
        #endregion
    }
}
*/