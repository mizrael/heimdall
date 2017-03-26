import * as React from "react";
import * as $ from "jquery";
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";

export interface ServicesArchiveState {
    services: Array<ServiceArchiveItem>;
}

export class ServicesArchive extends React.Component<{}, ServicesArchiveState> {
    constructor(props: any) {
        super(props);

        this.state = { services: [] };
    }

    private readServices() {
        let state = this.state,
            provider = new Services();
        provider.readServices().then(services => {
            state.services = services;
            this.setState(state);
        });
    }

    private renderService(service: ServiceArchiveItem) {
        return <tr key={service.name}>
            <td>{service.name}</td>
            <td>{service.active ? "yes" : "no"}</td>
            <td>{service.endpointsCount}</td>
            <td>{service.roundtripTime}</td>
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
        return <div className="services-wrapper">
            <table>
                <thead>
                    <tr>
                        <td>Name</td>
                        <td>Active</td>
                        <td>EndpointsCount</td>
                        <td>Roundtrip Time</td>
                    </tr>
                </thead>
                <tbody>{this.renderServices()}</tbody>
            </table>
        </div>;
    }
}