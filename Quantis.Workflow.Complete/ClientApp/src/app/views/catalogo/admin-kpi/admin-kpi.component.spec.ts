import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminKpiComponent } from './admin-kpi.component';

describe('AdminKpiComponent', () => {
    let component: AdminKpiComponent;
    let fixture: ComponentFixture<AdminKpiComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [AdminKpiComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(AdminKpiComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
