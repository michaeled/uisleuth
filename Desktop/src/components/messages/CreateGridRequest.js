export default class CreateGridRequest {
    constructor(parentId, columns, rows, columnSpacing, rowSpacing) {
        this.parentId = parentId;
        this.columns = columns;
        this.rows = rows;
        this.columnSpacing = columnSpacing;
        this.rowSpacing = rowSpacing;
        this.action = "CreateGridRequest";
    }
}