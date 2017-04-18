import * as React from "react";
import * as $ from "jquery";
import { Services } from "../services/services";
import { ServiceDetails, ServiceEndpoint, CreateService } from "../models/service";
import { Button, Modal, FormGroup, ControlLabel, FormControl } from "react-bootstrap";

export interface CreateServiceModalState {
    isLoading: boolean;
    show: boolean;
    form: CreateService
}

export interface CreateServiceModalProps {
    onClose: Function;
}

export class CreateServiceModal extends React.Component<CreateServiceModalProps, CreateServiceModalState> {
    constructor(props: CreateServiceModalProps) {
        super(props);

        this.state = { isLoading: false, show: false, form: { name: '', endpoint: '' } };
    }

    private open() {
        let state: CreateServiceModalState = this.state;
        state.show = true;
        this.setState(state);
    }

    private getValidationState() {
        return this.validate() ? 'success' : 'error';
    }

    private validate() {
        return (this.state.form.name.length > 0 && this.state.form.endpoint.length);
    }

    private handleChange(property: string, event: any) {
        let state: CreateServiceModalState = this.state;
        state.form[property] = event.target.value;
        this.setState(state);
    }

    private onSave(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();

        if (!this.validate())
            return;

        let state: CreateServiceModalState = this.state,
            services = new Services();
        state.isLoading = true;

        services.create(state.form)

        this.setState(state);
    }
    
    private onLoadingComplete(state: CreateServiceModalState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }
    
    public render() {
        return <div>
            <button onClick={() => this.open()}>Open</button>
            <Modal
            aria-labelledby='modal-label'
            show={this.state.show} 
            onHide={() => this.props.onClose()}>
                <Modal.Header closeButton>
                    <Modal.Title>Create new Service</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="modal-content-inner">
                        <form onSubmit={(e) => this.onSave(e)}>
                            <FormGroup validationState={this.getValidationState()}>
                                <ControlLabel>Service name</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.form.name}
                                    placeholder="Service name" required
                                    onChange={(e) => this.handleChange("name", e)}
                                />
                            </FormGroup>
                            <FormGroup validationState={this.getValidationState()}>
                                <ControlLabel>Endpoint url</ControlLabel>
                                <FormControl
                                    type="url"
                                    value={this.state.form.endpoint}
                                    placeholder="Endpoint"
                                    onChange={(e) => this.handleChange("endpoint", e)}
                                />
                            </FormGroup>

                            <Button type="submit">Submit</Button>
                        </form>
                    </div>
                </Modal.Body>
            </Modal>
        </div>;
    }
}