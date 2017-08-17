export default function EnterDirective() {
    return {
        link: function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if (event.which === 13) {
                    scope.$apply(function() {
                        scope.$eval(attrs.uisEnter);
                    });

                    event.preventDefault();
                }
            });
        }
    }
}