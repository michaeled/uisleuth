export default function FocusDirective ($timeout) {
    return {
        scope: { trigger: '=uisFocus' },
        link: function(scope, element) {
            scope.$watch('trigger', function(value) {
            if(value === true) { 
                element[0].focus();
                scope.trigger = false;
            }
      });
    }
  };
}