import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CatalogoKpiComponent } from './catalogo-kpi.component';

describe('CatalogoKpiComponent', () => {
    let component: CatalogoKpiComponent;
    let fixture: ComponentFixture<CatalogoKpiComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [CatalogoKpiComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(CatalogoKpiComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
