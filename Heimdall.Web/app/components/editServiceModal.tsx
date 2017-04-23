import * as React from "react";
import { Loading } from "./loading";
import { Services } from "../services/services";
import { ServiceDetails, ServiceEndpoint, RemoveEndpoint, AddEndpoint } from "../models/service";
import { Button, Modal, FormGroup, ControlLabel, FormControl } from "react-bootstrap";

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

    private addEndpoint() {
        //TODO
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

                <button onClick={() => this.addEndpoint()}>Add Endpoint</button>
                
            </form>;
        } else {
            content = <Loading></Loading>;
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

export interface EditServiceEndpointRowProps {
    serviceName: string;
    endpoint: ServiceEndpoint;
    onRemoved: Function;
}

export interface EditServiceEndpointRowState {
    isLoading: boolean;
}

export class EditServiceEndpointRow extends React.Component<EditServiceEndpointRowProps, EditServiceEndpointRowState> {
    constructor(props: EditServiceEndpointRowProps) {
        super(props);

        this.state = { isLoading: false };
    }

    private onRemove(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (!confirm("Are you sure?"))
            return;

        let me = this,
            state: EditServiceEndpointRowState = this.state,
            provider = new Services(),
            dto = new RemoveEndpoint(),
            onComplete = () => {
                state.isLoading = false;
                me.setState(state);
            }

        state.isLoading = true;
        this.setState(state);

        dto.endpoint = this.props.endpoint.url;
        dto.serviceName = this.props.serviceName;

        provider.deleteEndpoint(dto)
            .catch(() => {
                onComplete();
            }).then(() => {
                this.props.onRemoved();
                onComplete();
            });
    }

    public render() {
        if (!this.props.endpoint)
            return null;

        let actions = this.state.isLoading ? <span>processing...</span> :
            <button onClick={(e) => this.onRemove(e)}>Remove</button>;

        return <tr>
            <td>{this.props.endpoint.url}</td>
            <td>{actions}</td>
        </tr>;
    }
}