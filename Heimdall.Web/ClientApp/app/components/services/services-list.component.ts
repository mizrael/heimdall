import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ServicesService } from '../../services/services.service';
import { IServiceArchiveItem } from '../../models/service';

@Component({
    selector: 'services-list',
    templateUrl: './services-list.component.html'
})
export class ServicesListComponent {
    private items: IServiceArchiveItem[];

    constructor(private servicesService: ServicesService, private router:Router) {
        this.readItems();
    }

    private async readItems() {
        this.items = await this.servicesService.getAll();
    }
}
