import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { ModalModule } from 'ngx-modialog';
import { BootstrapModalModule } from 'ngx-modialog/plugins/bootstrap';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { LoadingComponent } from './components/shared/loading.component';
import { ErrorsComponent } from './components/shared/errors.component';
import { HomeComponent } from './components/home/home.component';

import { ServicesListComponent } from './components/services/services-list.component';
import { ServiceDetailsComponent } from './components/services/service-details.component';
import { AddEndpointComponent } from './components/services/add-endpoint.component';

import { ServicesService } from './services/services.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        LoadingComponent,
        ErrorsComponent,
        HomeComponent,
        ServicesListComponent,
        ServiceDetailsComponent, 
        AddEndpointComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ModalModule.forRoot(),
        BootstrapModalModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'services', pathMatch: 'full' },
            { path: 'services', component: HomeComponent },
            { path: 'services/details/:name', component: ServiceDetailsComponent },
            { path: '**', redirectTo: 'services' }
        ])
    ],
    entryComponents: [AddEndpointComponent],
    providers: [
        ServicesService
    ]
})
export class AppModuleShared {
}
