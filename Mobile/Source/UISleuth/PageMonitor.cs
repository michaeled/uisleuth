using System;
using System.Linq;
using Xamarin.Forms;

namespace UISleuth
{
    internal class PageMonitor
    {
        private readonly TimeSpan _interval;
        internal Page CurrentPage { get; set; }
        internal Page PreviousPage { get; private set; }
        internal bool PreviousPageIsModal { get; private set; }
        internal event EventHandler<PageChangedEventArgs> PageChanged;


        public PageMonitor(TimeSpan interval, Page currentPage)
        {
            _interval = interval;
            CurrentPage = currentPage;
            PreviousPage = null;
        }


        public void Start()
        {
            Device.StartTimer(_interval, () =>
            {
                if (Application.Current == null) return true;

                var currentPageIsModal = false;
                var pageChanged = false;

                var mainPage = Application.Current.MainPage;
                var mdPageRoot = mainPage as MasterDetailPage;

                if (mdPageRoot != null)
                {
                    mainPage = ((MasterDetailPage)mainPage).Detail;
                }

                var navPageRoot = mainPage as NavigationPage;
                var tabPageRoot = mainPage as TabbedPage;

                if (navPageRoot == null)
                {
                    var hasStack = mainPage.Navigation?.ModalStack?.Any();

                    if (hasStack.HasValue && hasStack.Value)
                    {
                        var modal = mainPage.Navigation?.ModalStack?.LastOrDefault();

                        if (modal != null)
                        {
                            if (CurrentPage != modal)
                            {
                                CurrentPage = modal;
                                pageChanged = true;
                                currentPageIsModal = true;
                            }
                        }
                    }
                }
                else
                {
                    var hasStack = navPageRoot.CurrentPage.Navigation?.ModalStack?.Any();

                    if (hasStack.HasValue && hasStack.Value)
                    {
                        var modal = navPageRoot.CurrentPage.Navigation?.ModalStack?.LastOrDefault() as NavigationPage;

                        if (modal != null)
                        {
                            if (modal.CurrentPage != CurrentPage)
                            {
                                CurrentPage = modal.CurrentPage;
                                pageChanged = true;
                                currentPageIsModal = true;
                            }
                        }
                    }
                    else if (navPageRoot.CurrentPage != CurrentPage)
                    {
                        if (navPageRoot.CurrentPage is TabbedPage)
                        {
                            var tabPageChild = (TabbedPage)navPageRoot.CurrentPage;
                            if (tabPageChild.CurrentPage != CurrentPage)
                            {
                                CurrentPage = tabPageChild.CurrentPage;
                                pageChanged = true;
                            }
                        }
                        else
                        {
                            CurrentPage = navPageRoot.CurrentPage;
                            pageChanged = true;
                        }
                    }
                }

                if (tabPageRoot != null)
                {
                    if (tabPageRoot.CurrentPage != CurrentPage)
                    {
                        CurrentPage = tabPageRoot.CurrentPage;
                        pageChanged = true;
                    }
                }

                // check for a popped page
                if (PreviousPageIsModal && !pageChanged)
                {
                    var modal = mainPage.Navigation?.ModalStack?.LastOrDefault();

                    // mult pages on stack
                    if (modal != null && CurrentPage != PreviousPage)
                    {
                        CurrentPage = modal;
                        pageChanged = true;
                    }

                    // last page on stack was popped.
                    if (modal == null && CurrentPage == PreviousPage)
                    {
                        CurrentPage = mainPage;
                        pageChanged = true;
                    }
                }

                if (pageChanged)
                {
                    OnPageChanged(new PageChangedEventArgs
                    {
                        NewPage = CurrentPage
                    });

                    PreviousPage = CurrentPage;
                    PreviousPageIsModal = currentPageIsModal;
                }

                return true;
            });
        }


        protected virtual void OnPageChanged(PageChangedEventArgs e)
        {
            PageChanged?.Invoke(this, e);
        }
    }
}
