export default class RotateCanvasObject {
    execute(editors, obj) {
        let rotation = editors.get("Rotation");

        obj.on("rotating", function(e) {
            let angle = this.get("angle");
            rotation.value = angle;
            rotation.changed();
        });
    }
}