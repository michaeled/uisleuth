export default function EscapeDirective() {
    return {
        link: function(scope, element, attrs) {
            element.bind("keydown keypress keyup", function(event) {
                if (event.which === 27) {
                    scope.$apply(function() {
                        scope.$eval(attrs.uisEsc);
                    });

                    event.preventDefault();
                }
            });
        }
    }
}