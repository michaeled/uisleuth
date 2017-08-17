declare var PubSub;
declare var JSONFormatter;

import ConnectionService from '../../connections/ConnectionService';
import GetBindingContextRequest from '../../messages/GetBindingContextRequest';
import MessageDispatcherService from '../../connections/MessageDispatcherService';

export default class BindingContextDialogService {
    public static $inject = ["ngDialog", "ConnectionService"];

    constructor(private ngDialog, private connection: ConnectionService) {
    }

    showDialog(node) {
        const self = this;

        this.ngDialog.open({
            template: './components/inspector/ultrabar/BindingContextDialog.htm',
            className: 'ngdialog-theme-default bc',
            controller: ["$scope", function($scope) {
                this.$scope = $scope;
                this.title = "BindingContext";

                this.close = function() {
                    PubSub.unsubscribe(this.gbToken);
                    this.$scope.closeThisDialog();
                }

                this.gbToken = PubSub.subscribe(MessageDispatcherService.GetBindingContextResponse, (e, d) => {
                    if (d.widgetId === node.id) {
                        const bc = JSON.parse(d.data);
                        const formatter = new JSONFormatter(bc, 1, {theme: 'dark'});
                        let target = document.getElementById('bc');

                        while (target.firstChild) {
                            target.removeChild(target.firstChild);
                        }

                        target.appendChild(formatter.render());
                        this.$scope.$apply();
                    }
                });
                
                if (node.id) {
                    self.connection.send(new GetBindingContextRequest(node.id));
                }
            }],

            controllerAs: "dlgctrl"
        });
    }
}