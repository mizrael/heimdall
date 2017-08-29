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

    public async onDelete(endpoint: IServiceEndpoint) {
        if (!confirm('Are you sure you want to remove the endpoint "' + endpoint.address + '" ?'))
            return;

        let dto = {
            address: endpoint.address,
            protocol: endpoint.protocol,
            serviceName: this.model.name
        };
        this.servicesService.deleteEndpoint(dto).then(() => {
            alert('endpoint deleted!');
        }).catch((err) => {
            alert('an error has occurred:\n' + err.message);
        });
    }
}
