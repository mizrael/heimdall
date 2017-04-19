import * as React from "react";
import * as ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";
import { ServicesArchiveItemRenderer } from "./servicesArchiveItemRenderer";
import { ServiceDetailsModal } from "./serviceDetailsModal";
import { CreateServiceModal } from "./createServiceModal";
import { Loading } from "./loading";

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

        let me = this;
        setInterval(function () {
            if (me.state.isLoading)
                return;
            me.readServices();
        }, 10 * 1000);
    }

    private readServices() {
        let state:ServicesArchiveRendererState = this.state,
            provider = new Services();

        state.isLoading = true;
        state.selectedService = null;
        state.services = [];
        this.setState(state);

        provider.readServices().then(services => {
            state.services = services;
            this.onLoadingComplete(state);
        });
    }

    private refreshAll() {
        if (!this.state.services || 0 == this.state.services.length) {
            return;
        }

        let state: ServicesArchiveRendererState = this.state,
            count = state.services.length,
            me = this,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        state.services.forEach(function (service) {
            provider.refresh(service.name)
                .then(function () {
                    if (--count == 0) {
                        state.isLoading = false;
                        me.setState(state);
                    }
                });
        });
    }
  
    private onLoadingComplete(state: ServicesArchiveRendererState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    private renderService(service: ServiceArchiveItem, index: number) {
        return <ServicesArchiveItemRenderer key={service.name}
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
            services = [...state.services];
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
        let content = null;

        if (!this.state.isLoading) {
            const items = this.state.services.map((s, i) => {
                return this.renderService(s, i);
            });

            content = <div className="services">
                <ul>
                    <li><CreateServiceModal onClose={() => this.readServices()} /></li>
                    <li><button onClick={() => this.readServices()}>Read</button></li>
                    <li><button onClick={() => this.refreshAll()}>Refresh all</button></li>
                </ul>

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
        } else {
            content = <Loading />;
        }

        return <div className="services-wrapper col-xs-12">
           {content}
        </div>;
    }
};


