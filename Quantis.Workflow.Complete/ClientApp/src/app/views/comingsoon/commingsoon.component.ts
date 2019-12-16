import { Component, OnInit } from '@angular/core';
import { AuthService, DashboardService } from '../../_services';
import { Router } from '@angular/router';

@Component({
    templateUrl: './commingsoon.component.html',
})
export class CommingsoonComponent implements OnInit {
    public currentUser: any;
    public permissions = [];
    constructor(
        private authService: AuthService,
        private dashboardService: DashboardService,
        private router: Router,
    ) { }

    ngOnInit() {
        this.authService.checkToken();
        this.currentUser = this.authService.getUser();
        this.permissions = this.currentUser.permissions;

        this.dashboardService.GetDefaultDashboardId().subscribe(result => {
            if (result !== -1) {
                this.router.navigate(['dashboard/public', result]);
            } else {
                this.router.navigate(['dashboard/list']);
            }
        });
    }
}
