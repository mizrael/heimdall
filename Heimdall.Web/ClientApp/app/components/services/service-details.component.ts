import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { ServicesService } from '../../services/services.service';
import { IServiceDetails, IServiceEndpoint } from '../../models/service';
import { Subscription } from "rxjs/Subscription";
import { Overlay } from 'ngx-modialog';
import { Modal } from 'ngx-modialog/plugins/bootstrap';

@Component({
    selector: 'service-details',
    templateUrl: './service-details.component.html'
})
export class ServiceDetailsComponent implements OnInit, OnDestroy {
    private model: IServiceDetails;

    private sub: Subscription;

    constructor(private route: ActivatedRoute, private servicesService: ServicesService, private modal: Modal) { }

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

    public addEndpoint() {
        
    }

    public onDeleteEndpoint(endpoint: IServiceEndpoint) {
        let message = 'Are you sure you want to remove the endpoint "' + endpoint.address + '" ?';
        this.modal.confirm()
            .title('Warning')
            .body(message)
            .isBlocking(true)
            .open().then(dlg => {
                dlg.result.then(val => {
                    this.deleteEndpoint(endpoint);
                }).catch(err => { });
            });
    }

    private deleteEndpoint(endpoint: IServiceEndpoint) {
        let dto = {
            address: endpoint.address,
            protocol: endpoint.protocol,
            serviceName: this.model.name
        };
        this.servicesService.deleteEndpoint(dto).then(() => {
            this.modal.alert()
                .title('Success')
                .body('endpoint deleted!');
        }).catch((err) => {
            let jsonErr = err.json(),
                message = 'an error has occurred:\n' + jsonErr.message;
            this.modal.alert()
                .title('Error')
                .body(message);
        });
    }

    public onDeleteService() {
        let message = 'Are you sure you want to remove the service "' + this.model.name + '" ?';
        this.modal.confirm()
            .title('Warning')
            .body(message)
            .isBlocking(true)
            .open().then(dlg => {
                dlg.result.then(val => {
                    this.deleteService();
                }).catch(err => { });
            });
    }

    private deleteService() {

    }
}
