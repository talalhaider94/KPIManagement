import { HttpHeaders } from '@angular/common/http';

export default class Headers {
    static setHeaders(type) {
        let headerObject = new HttpHeaders().set('Content-Type', 'application/json');
        headerObject = headerObject.append('Content-Type', 'application/json');
        headerObject = headerObject.append("Authorization", "Basic " + btoa("Quantis:WorkflowAPI"));
        headerObject = headerObject.append("Authorization-Type", "Preemptive");
        headerObject = headerObject.append('Access-Control-Allow-Headers', 'Content-Type');
        headerObject = headerObject.append('Access-Control-Allow-Methods', type);
        headerObject = headerObject.append('Access-Control-Allow-Origin', '*');
        return { headers: headerObject };
    }

    static setTokenHeaders(type) {
        let currentUser = JSON.parse(localStorage.getItem('currentUser'));
        let headerObject = new HttpHeaders();
        headerObject = headerObject.append('Content-Type', 'application/json');
        headerObject = headerObject.append("Authorization", "Basic " + btoa("Quantis:WorkflowAPI"));
        if(!!currentUser && !!currentUser.token) {
            headerObject = headerObject.append("AuthToken", currentUser.token);
        }
        headerObject = headerObject.append('Access-Control-Allow-Methods', type);
        return { headers: headerObject };
    }
}
