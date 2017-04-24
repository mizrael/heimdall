import * as React from "react";
import { Button, Modal, FormGroup, ControlLabel, FormControl } from "react-bootstrap";
import { Loading } from "./loading";
import { Services } from "../services/services";
import { ServiceDetails, ServiceEndpoint, AddEndpoint } from "../models/service";
import { AddServiceEndpointModal } from "./addServiceEndpointModal";
import { EditServiceEndpointRow } from "./editServiceEndpointRow";
   
export interface EditServiceModalState {
    isLoading: boolean;
    show: boolean;
    service: ServiceDetails
}

export interface EditServiceModalProps {
    onClose: Function;
    serviceName: string
}

export class EditServiceModal extends React.Component<EditServiceModalProps, EditServiceModalState> {
    constructor(props: EditServiceModalProps) {
        super(props);

        this.state = { isLoading: false, show: false, service: null };
    }

    private open() {
        let state: EditServiceModalState = this.state;
        state.show = true;
        this.setState(state);

        this.loadService();
    }

    private close() {
        let state: EditServiceModalState = this.state;
        state.show = false;
        this.setState(state);
        this.props.onClose();
    }

    private loadService() {
        let state: EditServiceModalState = this.state,
            provider: Services = null;

        state.service = null;

        if (this.props.serviceName) {
            provider = new Services();

            state.isLoading = true;

            provider.read(this.props.serviceName).then(service => {
                state.service = service;
                this.onLoadingComplete(state);
            }).catch(() => {
                this.onLoadingComplete(state);
            });
        }

        this.onLoadingComplete(state);
    }
    
    private onLoadingComplete(state: EditServiceModalState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    private renderEndpoints() {
        let me = this,
            state: EditServiceModalState = this.state,
            service = state.service;
    
        if (!service || !service.endpoints || 0 == service.endpoints.length)
            return null;

        return service.endpoints.map((endpoint, index) => {
            return <EditServiceEndpointRow key={endpoint.url}
                serviceName={this.props.serviceName} endpoint={endpoint}
                onRemoved={() => {
                    service.endpoints.splice(index, 1);
                    state.service = service;
                    me.setState(state);
                }
                } />;
        });
    }

    public render() {
        let content = null;
        if (!this.state.isLoading) {
            content = <form onSubmit={(e) => e.preventDefault() }>
                <FormGroup>
                    <ControlLabel>Service name</ControlLabel>
                    <FormControl
                        type="text"
                        readOnly
                        value={this.props.serviceName}
                    />
                </FormGroup>

                <table className="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>Url</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>{this.renderEndpoints()}</tbody>
                </table>

                <AddServiceEndpointModal serviceName={this.props.serviceName} onClose={() => this.loadService() } />
                
            </form>;
        } else {
            content = <Loading />;
        }

        return <div>
            <button onClick={() => this.open()}>Edit</button>
            <Modal
            aria-labelledby='modal-label'
            show={this.state.show} 
            onHide={() => { this.close() } }>
                <Modal.Header closeButton>
                    <Modal.Title>Edit Service {this.props.serviceName}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {content}
                </Modal.Body>
            </Modal>
        </div>;
    }
}