import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { mergeMap } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
    selector: 'app-chartdata',
    templateUrl: './chartdata.component.html',
    styleUrls: ['./chartdata.component.scss']
})
export class ChartDataComponent implements OnInit {
    dataFromWidgetsPage;
    id;
    value;

    constructor(
        private router: Router,
        private location: Location,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        console.log('In Chart Data');
        this.dataFromWidgetsPage = this.route.snapshot.queryParamMap['params'];
        this.location.replaceState('/dashboard/chartdata'); // remove query params from url after getting its value
        console.log('dataFromWidgetsPage: ', this.dataFromWidgetsPage);

        this.id = this.dataFromWidgetsPage.id;
        this.value = this.dataFromWidgetsPage.value;
    }
}
