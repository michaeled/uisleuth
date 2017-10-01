export default class PageSelectController {
    public static $inject = [
        "$scope",
    ];

    constructor (
        private $scope
    ) {

        console.trace("PageSelectController init.");
    }
}