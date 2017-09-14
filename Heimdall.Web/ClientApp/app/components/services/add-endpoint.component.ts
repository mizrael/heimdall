import { Component, ViewChild } from '@angular/core';
import { Modal, BSModalContext } from 'ngx-modialog/plugins/bootstrap';
import { overlayConfigFactory, ModalComponent, DialogRef, CloseGuard } from 'ngx-modialog';
import { IServiceDetails, IAddEndpoint } from '../../models/service';
import { IApiErrorResult} from '../../models/common';
import { ServicesService } from '../../services/services.service';

export class AddEndpointContext extends BSModalContext {
    public service: IServiceDetails;
    public onSaved: (endpoint: IAddEndpoint) => void;
}

@Component({
    selector: 'modal-content',
    templateUrl: './add-endpoint.html' 
})
export class AddEndpointComponent implements CloseGuard, ModalComponent<AddEndpointContext> {
    private context: AddEndpointContext;
    private model: IAddEndpoint;
    private isSaving: boolean;
    private errors: IApiErrorResult | null;

    constructor(public dialog: DialogRef<AddEndpointContext>, private servicesService: ServicesService) {
        this.context = dialog.context;
        this.model = { serviceName: this.context.service.name, address: '', protocol: '' };
        this.isSaving = false;
        dialog.setCloseGuard(this);        
    }

    public onSave() {
        this.isSaving = true;
        this.errors = null;

        this.servicesService.addEndpoint(this.model)
            .then( (successful) => {
                this.isSaving = false;
                if (this.context.onSaved)
                    this.context.onSaved(this.model);
            }).catch(error => {
                this.errors = error as IApiErrorResult;
                this.isSaving = false;
            });;
    }

    public close() {
        this.dialog.close();
    }
    
    beforeDismiss(): boolean {
        return true;
    }

    beforeClose(): boolean {
        return false;
    }
};