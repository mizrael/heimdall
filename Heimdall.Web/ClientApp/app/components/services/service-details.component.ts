import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { ServicesService } from '../../services/services.service';
import { IServiceDetails, IServiceEndpoint } from '../../models/service';
import { Subscription } from "rxjs/Subscription";

@Component({
    selector: 'service-details',
    templateUrl: './service-details.component.html'
})
export class ServiceDetailsComponent implements OnInit, OnDestroy {
    private model: IServiceDetails;

    private sub: Subscription;

    constructor(private route: ActivatedRoute, private servicesService: ServicesService) { }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            let name = params['name'];
            this.readItem(name);
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    private async readItem(name:string) {
        this.model = await this.servicesService.get(name);
    }

    public onDelete(endpoint: IServiceEndpoint) {
        console.log(endpoint);
    }
}
