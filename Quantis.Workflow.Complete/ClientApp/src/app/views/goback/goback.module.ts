import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { CommonModule } from '@angular/common';
import { GobackComponent } from './goback.component';
import { GobackRoutingModule } from './goback-routing.module';

@NgModule({
    imports: [
        FormsModule,
        GobackRoutingModule,
        ChartsModule,
        BsDropdownModule,
        CommonModule,
        ButtonsModule.forRoot()
    ],
    declarations: [GobackComponent]
})
export class GobackModule { }
