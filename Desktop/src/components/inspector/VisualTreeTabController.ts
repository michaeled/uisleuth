declare var PubSub;

import MessageDispatcherService from "../connections/MessageDispatcherService";
import GetWidgetPropertiesRequest from '../messages/GetWidgetPropertiesRequest'
import GetWidgetEventsRequest from '../messages/GetWidgetEventsRequest'
import GetAttachedPropertiesRequest from '../messages/GetAttachedPropertiesRequest'
import GetVisualTreeRequest from '../messages/GetVisualTreeRequest';
import ConnectionService from '../connections/ConnectionService';
import { getNodeOptions } from './dnd/VisualTreeNodeOptions';

export default class VisualTreeTabController {
    public static $inject = ["$scope", "ConnectionService"];
    public static RefreshNodeData = "VTC.RefreshNodeData";
    public static NewNodeClicked = "VTC.NewNodeClicked";
    public static RefreshVisualTreeAndSelectNode = "VTC.RefreshVisualTreeAndSelectNode";
    public static SelectNode = "VTC.SelectNode";

    public tree: any = null;
    public firstStart: boolean = false;
    public selected: any = null;
    public options: any = null;

    private selectAfterRefresh: any = null;

    constructor(private $scope, public connection: ConnectionService) {

        let gvtToken = PubSub.subscribe(MessageDispatcherService.GetVisualTreeResponse, (e, d) => {
            this.transform(d);
            this.reselectNode();
            $scope.$apply();
        });

        // refresh current selected node
        let rndToken = PubSub.subscribe(VisualTreeTabController.RefreshNodeData, (e, d) => {
            if (!this.selected) return;

            this.requestNodeData(this.selected);
            $scope.$apply();
        });

        // a visual element has been moved; refresh all data
        let sprToken = PubSub.subscribe(MessageDispatcherService.SetParentResponse, (e, d) => {
            this.selectAfterRefresh = d.childId;
            this.connection.send(new GetVisualTreeRequest());
        });

        let rasToken = PubSub.subscribe(VisualTreeTabController.RefreshVisualTreeAndSelectNode, (e, d) => {
            // widgetid is passed
            this.selectAfterRefresh = d;
            this.connection.send(new GetVisualTreeRequest());
        });

        let snToken = PubSub.subscribe(VisualTreeTabController.SelectNode, (e, d) => {
            // d is the node model
            this.onNodeClicked(d);
        });

        $scope.$on("$destroy", () => {
            this.firstStart = false;
            PubSub.unsubscribe(gvtToken);
            PubSub.unsubscribe(rndToken);
            PubSub.unsubscribe(sprToken);
            PubSub.unsubscribe(rasToken);
            PubSub.unsubscribe(snToken);
        });

        $scope.$on("uisleuth.inspector.ready", () => {
            this.firstStart = true;
        });

        this.options = getNodeOptions(this);
        console.trace("visual tree controller init.");
    }

    public keypress($event, node): void {
        // tab key
        if ($event.keyCode === 9) {
            this.onNodeClicked(node);
            return;
        }
    }

    public onNodeClicked(node): void {
        // prevent redundant multi requests
        if (this.selected === node) {
            return;
        }

        this.selected = node;
        this.requestNodeData(this.selected);
        PubSub.publish(VisualTreeTabController.NewNodeClicked, { tree: this.tree[0], selected: this.selected });
    }

    public hasChildren(node): boolean {
        if (node == undefined) return false;
        if (node.children == undefined) return false;
        if (node.children.length == undefined) return false;

        return node.children.length > 0;
    }

    private requestNodeData(node): void {
        let propReq = new GetWidgetPropertiesRequest(node.id);
        let evtsReq = new GetWidgetEventsRequest(node.id);
        let apReq = new GetAttachedPropertiesRequest(node.id);

        this.connection.send(propReq);
        this.connection.send(evtsReq);
        this.connection.send(apReq);
    }

    private reselectNode(): void {
        if (this.tree.length > 0) {
            let selectNode = null;

            if (this.selectAfterRefresh == null) {
                selectNode = this.tree[0];
            } else {
                let match = findNode(this.selectAfterRefresh, this.tree[0]);

                if (match) {
                    selectNode = match;
                } else {
                    selectNode = this.tree[0];
                }

                this.selectAfterRefresh = null;
            }

            if (selectNode) {
                this.onNodeClicked(selectNode);
            }
            
        } else {
            console.error("No visual elements returned");
        }
    }

    private transform(vt): void {
        this.tree = [vt.root];
        setNodeParent(vt.root);
    }
}

function setNodeParent(parent) {
    if (parent.children.length === 0) return;

    for (const child of parent.children) {
        child.parent = parent;
        setNodeParent(child);
    }
}

function findNode(id, currentNode) {
    let i,
        currentChild,
        result;

    if (id == currentNode.id) {
        return currentNode;
    } else {
        for (i = 0; i < currentNode.children.length; i += 1) {
            currentChild = currentNode.children[i];

            // Search in the current child
            result = findNode(id, currentChild);

            // Return the result if the node has been found
            if (result !== false) {
                return result;
            }
        }

        // The node has not been found and we have no more options
        return false;
    }
}