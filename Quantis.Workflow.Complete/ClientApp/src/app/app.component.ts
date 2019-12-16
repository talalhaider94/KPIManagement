import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from './_services/auth.service';

@Component({
    selector: 'body',
    template: '<router-outlet></router-outlet>'
})
export class AppComponent implements OnInit {
    constructor(
        private router: Router,
        private authService: AuthService

    ) { }

    ngOnInit() {
        this.router.events.subscribe((evt) => {
            if (!(evt instanceof NavigationEnd)) {
                return;
            }
            //console.log(evt.url);
            //if (evt.url !== '/forget') {        //remove when logout on 401 is implemented
            //  this.authService.checkSession();  //remove when logout on 401 is implemented
            //}                                   //remove when logout on 401 is implemented
            window.scrollTo(0, 0);
        });
    }
}
