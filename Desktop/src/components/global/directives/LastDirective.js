export default function LastDirective() {
    return {
        link: function(scope, element, attrs) {
            if (scope.$last) {
                scope.$eval(attrs.uisLast);
            }
        }
    }
}