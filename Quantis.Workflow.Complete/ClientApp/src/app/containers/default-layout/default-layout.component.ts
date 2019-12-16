import { Component, OnDestroy, Inject, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { navItems } from '../../_nav';
import { pageVersion } from '../../_page-versions';
import { AuthService } from '../../_services';
import { Router, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter } from 'rxjs/operators';
import { ObservableLike } from 'rxjs';
import { DashboardService, EmitterService } from '../../_services';
import { WidgetModel, DashboardModel } from "../../_models";

@Component({
    selector: 'app-dashboard',
    templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent implements OnDestroy, OnInit {
    permittedMenuItems = [];
    public navItems = [];
    public sidebarMinimized = true;
    private changes: MutationObserver;
    public element: HTMLElement;
    private currentUrl = '0.0.1';
    public currentVerion = '0.0.1';
    public returnedNode: any;
    currentUser: any;
    loading: boolean = true;
    loadingDashboard: boolean = false;
    dashboardCollection: DashboardModel[];
    public showLandingPage: any;
    showPowerBI=0;
    public showAboutPage: boolean = false;

    constructor(
        private toastr: ToastrService,
        private authService: AuthService,
        private router: Router,
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        @Inject(DOCUMENT) _document?: any,
    ) {
        this.currentUser = this.authService.getUser();
        this.filterMenuByPermission(navItems, this.currentUser.permissions, this.permittedMenuItems);
        this.navItems = this.permittedMenuItems;

        this.changes = new MutationObserver((mutations) => {
            this.sidebarMinimized = _document.body.classList.contains('sidebar-minimized');
            if (_document.body.classList.contains('brand-minimized')) {
                $('body').removeClass('brand-minimized'); //to not minimize the brand
            }
        });
        this.element = _document.body;
        this.changes.observe(<Element>this.element, {
            attributes: true,
            attributeFilter: ['class']
        });
    }

    ngOnInit() {
        this.showPowerBI=0;
        let x = JSON.parse(localStorage.getItem('currentUser'));
        let perm = x.permissions;
        if(perm.indexOf('VIEW_LINK_POWERBI') > -1){
            this.showPowerBI=1;
        }

        this.currentUser = this.authService.getUser();
        this.router.events.pipe(
            filter((event: any) => event instanceof NavigationEnd)
        ).subscribe(x => {
            this.currentUrl = x.url;
            //this.findUrlDataByName(this.navItems, this.currentUrl);
            this.currentVerion = '0.0.1';
            if (pageVersion[this.currentUrl]) {
                this.currentVerion = pageVersion[this.currentUrl];
            } else {
                this.currentVerion = '0.0.1';
            }
        });
        this.loadingSpinnerSubscription();
        this.getAllDashboards();

        this.dashboardService.getLandingPageInfo().subscribe(data => {
            this.showLandingPage = data.showlandingpage;
            //console.log("Landing Page Info -> ",data.showlandingpage);
        });

        // if user is admin and can see about link
        this.showAboutPage = this.authService.checkIfPermissionKeyExists('VIEW_ADMIN_PAGE_VERSION');
    }

    ngOnDestroy(): void {
        this.changes.disconnect();
    }

    logout() {
        this.authService.logout().subscribe(data => {
            this.authService.removeUser();
            this.toastr.success('Success', 'Logout eseguito con successo.');
            this.router.navigate(['/login']);
        });
    }

    filterMenuByPermission(navItems, permissions, permittedMenu) {
        if (navItems) {
            navItems.forEach((item: any, index) => {
                if (item.UIVersion) {
                    item.name = this.currentUser.uiversion; //UI Version taken on login
                }
                let isExist: boolean = item.title || item.divider || item.key == 'alwaysShow' || this.checkArrays(item.key === undefined ? ['$#%^&'] : typeof (item.key) === 'string' ? [item.key] : item.key, permissions);
                let cloneItem = { ...{}, ...item };
                if (isExist) { // || item.title || item.divider || item.key == 'alwaysShow'
                    cloneItem.children = [];
                    permittedMenu.push(cloneItem);
                }
                if (item.children) {
                    this.filterMenuByPermission(item.children, permissions, cloneItem.children);
                } else {
                    delete cloneItem.children;
                }
            });
        }
    }

    checkArrays(arr1, arr2) {
        let isExist = false;
        arr1.forEach((item: any) => {
            if (arr2) {
                if (arr2.indexOf(item) > -1) {
                    isExist = true;
                }
            }
        });
        return isExist;
    }

    findUrlDataByName(itemsArray, url) {
        if (itemsArray) {
            itemsArray.forEach((item: any) => {
                if (item.url === url) {
                    this.returnedNode = item;
                }
                if (item.children) {
                    this.findUrlDataByName(item.children, url);
                } else {
                }
            });
        }
    }

    dashboardList() {
        //this.router.navigate(['/dashboard/list']);
        let x = JSON.parse(localStorage.getItem('currentUser'));
        let dashboardid = x.defaultdashboardid;

        if(dashboardid==-1){
            this.router.navigate(['/dashboard/landingpage']);
        }else if(dashboardid!=-1){
            this.router.navigate(['/dashboard/public', dashboardid]);
        }else{
            this.router.navigate(['dashboard/nodashboard']);
        }
        //console.log('dashboardid: ',dashboardid);
    }

    dashboardNavigation(id) {
        this.router.navigate(['/dashboard/dashboard', id]);
    }

    standardDashboardNavigation() {
        this.router.navigate(['/dashboard/landingpage']);
    }

    loadingSpinnerSubscription() {
        this.emitter.getData().subscribe(data => {
            if (data.type === 'loading') {
                if (this.loading !== data.loading) {
                    setTimeout(() => {
                        this.loading = data.loading;
                    })
                }
            }
        });
    }

    getAllDashboards() {
        this.dashboardService.getDashboards().subscribe(dashboards => {
            this.emitter.loadingStatus(false);
            this.dashboardCollection = dashboards.filter(dashboard => dashboard.isactive);
        }, error => {
            this.toastr.error('Error while loading dashboards');
            this.emitter.loadingStatus(false);
        });
    }

    dashboardSwitch(switchValue, id: number) {
        this.loadingDashboard = true;
        if (id == -1) {
            this.dashboardService.selectLandingPage().subscribe(data => {
                this.loadingDashboard = false;
                this.toastr.success('Success', 'Dashboard successfully set as stanadard.');
                this.getAllDashboards();
            }, error => {
                this.loadingDashboard = false;
                console.error('dashboardSwitch', error);
                this.toastr.error('Error', 'Unable to set standard dashboard.');
            })
        } else {
            this.dashboardService.setDefaultDashboard(id).subscribe(result => {
                this.loadingDashboard = false;
                console.log('dashboardSwitch', result);
                this.toastr.success('Success', 'Dashboard successfully set as default.');
                this.getAllDashboards();
            }, error => {
                this.loadingDashboard = false;
                console.error('dashboardSwitch', error);
                this.toastr.error('Error', 'Unable to set default dashboard.');
            })
        }
    }

    saveLandingPage() {
        this.dashboardService.selectLandingPage().subscribe(data => {
        });
    }

    powerBI(){
        window.open("https://rapid.posteitaliane.it", "_blank", "toolbar=yes,scrollbars=yes,resizable=yes,width=1200,height=1200");
    }
}
