import * as React from "react";
import * as $ from "jquery";
import { Services } from "../services/services";
import { ServiceDetails, ServiceEndpoint } from "../models/service";
import { Button, Modal } from "react-bootstrap";

export interface ServiceDetailsModalState {
    model: ServiceDetails;
    isLoading: boolean;
}

export interface ServiceDetailsModalProps {
    serviceName: string;
    show: boolean;
    onClose: Function;
}

export class ServiceDetailsModal extends React.Component<ServiceDetailsModalProps, ServiceDetailsModalState> {
    constructor(props: ServiceDetailsModalProps) {
        super(props);
        
        this.state = { model: null, isLoading: false };
    }

    private onShow() {
        let state: ServiceDetailsModalState = this.state,
            provider: Services = null;

        if (this.props.serviceName) {
            provider = new Services();

            state.isLoading = true;
            
            provider.read(this.props.serviceName).then(service => {
                state.model = service;
                this.onLoadingComplete(state);
            }).catch(() => {
                this.onLoadingComplete(state);
            });
        } else {
            state.isLoading = false;
            state.model = null;
        }

        this.onLoadingComplete(state);
    }

    private onLoadingComplete(state: ServiceDetailsModalState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    private renderModel() {
        if (!this.state.model)
            return;
        let service = this.state.model,
            bestEndpoint = null,
            endpoints = null;

        if (service.bestEndpoint) {
            bestEndpoint = <div>
                <span>best endpoint: {service.bestEndpoint.url} , roundtrip time: {service.bestEndpoint.roundtripTime}ms</span>
            </div>;
        }

        if (service.endpoints && 0 != service.endpoints.length) {
            endpoints = this.renderEndpoints(service.endpoints);
        } else {
            endpoints = <div>There are no available endpoints at the moment.</div>;
        }

        return <div>
            <h4 id='modal-label'>{service.name}</h4>
            {bestEndpoint}
            {endpoints}
        </div>;
    }

    private renderEndpoints(endpoints: Array<ServiceEndpoint>) {
        return <table className="table table-striped table-hover">
            <thead>
                <tr>
                    <th>Url</th>
                    <th>Active</th>
                    <th>Roundtrip Time</th>
                </tr>
            </thead>
            <tbody>{endpoints.map(s => {
                return this.renderEndpoint(s);
            })}</tbody>
        </table>;
    }

    private renderEndpoint(endpoint: ServiceEndpoint) {
        return <tr key={Math.random()}>
            <td>{endpoint.url}</td>
            <td>{endpoint.active ? 'yes' : 'no'}</td>
            <td>{endpoint.active ? endpoint.roundtripTime + 'ms' : '-'}</td>
        </tr>;
    }

    public render() {
        return <Modal
            aria-labelledby='modal-label'
            show={this.props.show}
            onShow={ () => this.onShow() }
            onHide={() => this.props.onClose()}>
                <Modal.Header closeButton>
                    <Modal.Title>Service Details: <span>{this.props.serviceName}</span></Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {this.renderModel()}
                </Modal.Body>
        </Modal>;
    }
}