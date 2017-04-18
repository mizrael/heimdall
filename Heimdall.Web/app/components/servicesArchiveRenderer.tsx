import * as React from "react";
import * as ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";
import { ServicesArchiveItemRenderer } from "./servicesArchiveItemRenderer";
import { ServiceDetailsModal } from "./serviceDetailsModal";
import { CreateServiceModal } from "./createServiceModal";

export interface ServicesArchiveRendererState {
    services: Array<ServiceArchiveItem>;
    selectedService: ServiceArchiveItem;
    openDetails: boolean;
    isLoading: boolean;
}

export class ServicesArchiveRenderer extends React.Component<{}, ServicesArchiveRendererState> {
    constructor(props: any) {
        super(props);

        this.state = { services: [], isLoading: false, selectedService: null, openDetails: false };
    }

    private readServices() {
        let state:ServicesArchiveRendererState = this.state,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        provider.readServices().then(services => {
            state.services = services;
            this.onLoadingComplete(state);
        });
    }
  
    private onLoadingComplete(state: ServicesArchiveRendererState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    private renderService(service: ServiceArchiveItem, index: number) {
        return <ServicesArchiveItemRenderer key={index}
            rowIndex={index} model={service}
            onDeleted={(index: number) => this.onServiceDeleted(index)} 
            onSelect={(u: any) => this.onSelectService(u)} />;
    }

    private selectService(serviceItem: ServiceArchiveItem) {
        let state: ServicesArchiveRendererState = this.state;
        state.selectedService = serviceItem;
        state.openDetails = (null != serviceItem);
        this.setState(state);
    }

    private onServiceDeleted(index: number) {
        let state: ServicesArchiveRendererState = this.state,
            services = state.services.slice();
        services.splice(index, 1);
        state.services = services;
        this.setState(state);
    }

    private onSelectService(serviceItem: ServiceArchiveItem) {
        this.selectService(serviceItem);
    }

    private getSelectedServiceName() {
        return (null != this.state.selectedService) ? this.state.selectedService.name : '';
    }

    public componentDidMount() {
        this.readServices();
    }

    public render() {
        const items = this.state.services.map((s, i) => {
            return this.renderService(s, i);
        });

        return <div className="services-wrapper col-xs-12">
            <CreateServiceModal onClose={() => this.readServices()} />
            <table className="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Active</th>
                        <th># Endpoints</th>
                        <th>Roundtrip Time</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <ReactCSSTransitionGroup
                    transitionName="example"
                    transitionEnterTimeout={500}
                    transitionLeaveTimeout={300}
                    component="tbody">
                    {items}
                </ReactCSSTransitionGroup>
            </table>

            <ServiceDetailsModal serviceName={this.getSelectedServiceName()} show={this.state.openDetails} onClose={() => this.selectService(null)} />
            
        </div>;
    }
};