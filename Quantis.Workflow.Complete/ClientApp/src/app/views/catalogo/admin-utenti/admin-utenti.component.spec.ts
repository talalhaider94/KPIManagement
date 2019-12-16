import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminUtentiComponent } from './admin-utenti.component';

describe('AdminUtentiComponent', () => {
    let component: AdminUtentiComponent;
    let fixture: ComponentFixture<AdminUtentiComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [AdminUtentiComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(AdminUtentiComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
