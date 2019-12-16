import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, EMPTY } from 'rxjs';
import { map, catchError, retry, shareReplay } from 'rxjs/operators';
import * as sha256 from 'sha256';
import { environment } from '../../environments/environment';
import Headers from '../_helpers/headers';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject: BehaviorSubject<any>;
    public currentUser: Observable<any>; // Danial: To use for reactively updating UI login.
    // Danial TODO: replace any with user model in future
    constructor(private http: HttpClient, private router: Router) {
        this.currentUserSubject = new BehaviorSubject<any>(this.getUser());
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): any {
        return this.currentUserSubject.value;
    }

    login(username: string, password: string): Observable<any> {
        const hashedPassword: string = sha256('p4ssw0rd' + sha256(password));
        // Danial TODO: move endpoint into separat constant file
        const loginEndPoint = `${environment.API_URL}/Data/Login?username=${username}&password=${hashedPassword}`;
        // Danial TODO: move headers code into custom HTTP Interceptor class
        return this.http.get<any>(loginEndPoint, Headers.setHeaders('GET'))
            .pipe(retry(2), map(user => {
                if (!!user && user.token) {
                    user.last_action = Date.now(); // temp
                    console.log(user);
                    localStorage.setItem('currentUser', JSON.stringify(user));
                    this.currentUserSubject.next(user);
                }
                return user;
            }),
                // catchError(() => {
                //   return EMPTY
                // }),
                shareReplay()
            );
    }

    logout() {
        const logoutEndPoint = `${environment.API_URL}/data/logout`;
        var logout = this.http.get(logoutEndPoint, Headers.setTokenHeaders('GET'));
        this.currentUserSubject.next(null);
        localStorage.removeItem('currentUser');
        return logout;
    }

    getUser() {
        const currentUser = localStorage.getItem('currentUser');
        return currentUser ? JSON.parse(currentUser) : false;
    }

    removeUser() {
        return localStorage.removeItem('currentUser');
    }

    isLoggedIn(): boolean {
        return !!this.getUser();
    }

    checkSession() { //temporary function while implementing logout on 401 error //remove in app.component.ts
        let user = JSON.parse(localStorage.getItem('currentUser'));
        if (user) {
            let last_action = user.last_action;
            if ((Date.now() - last_action) <= 1800000) {
                let new_action = Date.now();
                user.last_action = new_action;
                localStorage.setItem('currentUSer', JSON.stringify(user));
                return true
            }
        }
        //console.log('sessione scaduta')
        this.logout();
        this.router.navigate(['/login']);
        return false
    }
    checkLogin(): Observable<any> {
      /*const checkLoginEndPoint = `${environment.API_URL}/Information/CheckLogin`;
      this.http.get<any>(`${environment.API_URL}/Data/GetHeaders`, Headers.setTokenHeaders('GET') ).subscribe(resp => {
        console.log(resp.headers.get('Cookie'));
        console.log(resp.headers);
        console.log()
        });*/
      //return this.http.get(checkLoginEndPoint, Headers.setTokenHeaders('GET'));
      return this.http.get<any>(`${environment.API_URL}/Data/GetHeaders`, Headers.setTokenHeaders('GET')).pipe(retry(2), map(user => {
        if (!!user && user.token) {
          user.last_action = Date.now(); // temp
          console.log(user);
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
        }
        return user;
      }),
        // catchError(() => {
        //   return EMPTY
        // }),
        shareReplay()
      );
    }
    checkToken() {
        this.checkLogin().subscribe((data: any) => {
            console.log('logged');
        }, error => {
            this.logout();
            this.router.navigate(['/login']);
            return false
        })
    }

    resetPassword(username, email): Observable<any> {
        const resetPasswordEndPoint = `${environment.API_URL}/Data/ResetPassword?username=${username}&email=${email}`;
        return this.http.get(resetPasswordEndPoint, Headers.setHeaders('GET'))
            .pipe(map(data => {
                return data;
            }))
    }

    getUserPermissions() {
        const loggedInUser = this.currentUserValue || this.getUser();
        let permissionsArray = [];
        if(loggedInUser) {
            permissionsArray = loggedInUser.permissions;
        }
        return permissionsArray;

    }

    checkIfPermissionKeyExists(key: string): boolean {
        let permission = false;
        const userPermissions = this.getUserPermissions();
        if(userPermissions && userPermissions.length) {
            const formPermission = userPermissions.find(permission => permission === key);
            permission = formPermission ? true : false;
        }
        return permission;
    }
}
