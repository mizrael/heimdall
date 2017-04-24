import * as React from "react";
import { Button, Modal, FormGroup, ControlLabel, FormControl } from "react-bootstrap";
import { Loading } from "./loading";
import { Services } from "../services/services";
import { ServiceDetails, ServiceEndpoint, AddEndpoint } from "../models/service";
import { EditServiceEndpointRow } from "./editServiceEndpointRow";

export interface AddServiceEndpointModalState {
    isLoading: boolean;
    show: boolean;
    model: {
        endpointUrl: string
    }
}

export interface AddServiceEndpointModalProps {
    onClose: Function;
    serviceName: string
}

export class AddServiceEndpointModal extends React.Component<AddServiceEndpointModalProps, AddServiceEndpointModalState> {
    constructor(props: AddServiceEndpointModalProps) {
        super(props);

        this.state = { isLoading: false, show: false, model: { endpointUrl: "" } };
    }
    
    private open() {
        let state: AddServiceEndpointModalState = this.state;
        state.show = true;
        this.setState(state);
    }

    private close() {
        let state: AddServiceEndpointModalState = this.state;
        state.show = false;
        state.isLoading = false;
        state.model.endpointUrl = "";
        this.setState(state);
        this.props.onClose();
    }
    
    private onLoadingComplete(state: AddServiceEndpointModalState) {
        state = state || this.state;
        state.isLoading = false;
        
        this.setState(state);
    }

    private handleChange(property: string, event: any) {
        let state: AddServiceEndpointModalState = this.state;
        state.model[property] = event.target.value;
        this.setState(state);
    }

    private validate() {
        return (this.state.model.endpointUrl.length > 0);
    }

    private getValidationState() {
        return this.validate() ? 'success' : 'error';
    }

    private onSave(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();

        if (!this.validate())
            return;

        let me = this,
            state: AddServiceEndpointModalState = this.state,
            services = new Services(),
            dto = new AddEndpoint(this.props.serviceName, state.model.endpointUrl);
        state.isLoading = true;
        
        services.addEndpoint(dto).then(function (success: boolean) {
            let message = (success) ? 'Saved!' : 'An error has occurred, please try again later';
            alert(message);
            me.close();
        });

        this.setState(state);
    }

    private renderContent() {
        let content = null;
        if (!this.state.isLoading) {
            content = <form onSubmit={(e) => this.onSave(e)}>
                <FormGroup validationState={this.getValidationState()}>
                    <ControlLabel>Service name</ControlLabel>
                    <FormControl
                        type="url"
                        value={this.state.model.endpointUrl}
                        placeholder="Endpoint url" required
                        onChange={(e) => this.handleChange("endpointUrl", e)}
                    />
                </FormGroup>

                <Button type="submit">Submit</Button>
            </form>;
        } else {
            content = <Loading></Loading>;
        }
        return content;
    }

    public render() {
        return <div>
            <button onClick={() => this.open()}>Add Endpoint</button>
            <Modal
            aria-labelledby='modal-label'
            show={this.state.show} 
            onHide={() => { this.close() } }>
                <Modal.Header closeButton>
                    <Modal.Title>Add New Endpoint to Service {this.props.serviceName}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {this.renderContent()}
                </Modal.Body>
            </Modal>
        </div>;
    }
}