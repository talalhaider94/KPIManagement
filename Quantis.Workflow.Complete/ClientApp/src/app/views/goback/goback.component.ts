import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './goback.component.html',
})
export class GobackComponent implements OnInit {
    constructor(
        private loc: Location,
        private router: Router,
        private activatedRoute: ActivatedRoute
    ) {
        this.goBack();
    }

    ngOnInit() {
        //window.open('https://rapid.posteitaliane.it/','','width=1000,height=900');
    }

    goBack() {
        this.loc.back();
    }
}
