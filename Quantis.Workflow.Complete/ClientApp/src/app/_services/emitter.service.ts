import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class EmitterService {
    private subjectEvent = new Subject();

    constructor() { }

    sendNext(data: any) {
        this.subjectEvent.next(data);
    }

    clear() {
        this.subjectEvent.next();
    }

    getData(): Observable<any> {
        return this.subjectEvent.asObservable();
    }

    loadingStatus(loading) {
        this.sendNext({ type: 'loading', loading });
    }
}
