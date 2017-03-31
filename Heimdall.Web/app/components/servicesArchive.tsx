import * as React from "react";
import * as $ from "jquery";
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";

export interface ServicesArchiveState {
    services: Array<ServiceArchiveItem>;
    isLoading: boolean;
}

export class ServicesArchive extends React.Component<{}, ServicesArchiveState> {
    constructor(props: any) {
        super(props);

        this.state = { services: [], isLoading: false };
    }

    private readServices() {
        let state:ServicesArchiveState = this.state,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        provider.readServices().then(services => {
            state.services = services;
            this.onLoadingComplete(state);
        });
    }
    
    private viewServiceDetails(e:any, serviceName:string) {
        e.preventDefault();

        let state: ServicesArchiveState = this.state,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        provider.read(serviceName).then(service => {
            this.onLoadingComplete(state);

            if (!service)
                return;
        }).catch(() => {
            this.onLoadingComplete(state);
        });
    }

    private refreshService(e: any, serviceName: string) {
        e.preventDefault();

        let state: ServicesArchiveState = this.state,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        provider.refresh(serviceName).catch(() => {
            this.onLoadingComplete(state);
        }).then(() => {
            this.onLoadingComplete(state);
        });
    }

    private onLoadingComplete(state: ServicesArchiveState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    private renderService(service: ServiceArchiveItem) {
        let actions = (this.state.isLoading) ? <td>processing....</td> : 
            <td>
                <button data-toggle="modal" data-target="#myModal" onClick={e => this.viewServiceDetails(e, service.name)}>View</button>
                <button onClick={e => this.refreshService(e, service.name)}>Refresh</button>
            </td>;

        return <tr key={service.name}>
            <td>{service.name}</td>
            <td>{service.active ? "yes" : "no"}</td>
            <td>{service.endpointsCount}</td>
            <td>{service.roundtripTime}</td>
            {actions}    
        </tr>;
    }

    private renderServices() {
        return this.state.services.map(s => {
            return this.renderService(s);
        });
    }

    public componentDidMount() {
        this.readServices();
    }

    public render() {
        return <div className="services-wrapper col-xs-12">
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
                <tbody>{this.renderServices()}</tbody>
            </table>

            <div className="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 className="modal-title" id="myModalLabel">Modal title</h4>
                        </div>
                        <div className="modal-body">
                            lorem ipsum
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                            <button type="button" className="btn btn-primary">Save changes</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }
}