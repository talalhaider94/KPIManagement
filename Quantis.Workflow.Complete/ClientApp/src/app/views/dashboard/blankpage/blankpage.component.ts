import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
declare var $;
var $this;

@Component({
    templateUrl: './blankpage.component.html'
})

export class BlankPageComponent implements OnInit {
    constructor(
    ) {
        $this = this;
    }

    ngOnInit() {}

}
