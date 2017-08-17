import MessageDispatcherService from "../connections/MessageDispatcherService";

export default class EventsTabController {
    constructor($scope) {
        this.events = [];

        let gerToken = PubSub.subscribe(MessageDispatcherService.GetWidgetEventsResponse, (e, d) => {
            this.events = d.events;
            $scope.$apply();
        });

        $scope.$on("$destroy", () => {
            PubSub.unsubscribe(gerToken);
        });

        console.trace("events controller init.");
    }
}

EventsTabController.$inject = ["$scope"];