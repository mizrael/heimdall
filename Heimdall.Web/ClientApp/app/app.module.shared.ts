import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';

import { ServicesListComponent } from './components/services/services-list.component';
import { ServiceDetailsComponent } from './components/services/service-details.component';

import { ServicesService } from './services/services.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ServicesListComponent,
        ServiceDetailsComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'services', pathMatch: 'full' },
            { path: 'services', component: HomeComponent },
            { path: 'services/details/:name', component: ServiceDetailsComponent },
            { path: '**', redirectTo: 'services' }
        ])
    ],
    providers: [
        ServicesService
    ]
})
export class AppModuleShared {
}
