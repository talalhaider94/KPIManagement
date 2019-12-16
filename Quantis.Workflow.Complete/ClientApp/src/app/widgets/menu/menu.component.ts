import { Component, OnInit, Input } from '@angular/core';
import { WidgetModel } from "../../_models";

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html',
    styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
    @Input()
    widgetCollection: WidgetModel[];

    constructor() { };

    ngOnInit(): void {
    }

    onDrag(event, identifier) {
        event.dataTransfer.setData('widgetIdentifier', identifier);
    }
}
