﻿import * as React from "react";
import { Services } from "../services/services";
import { ServiceEndpoint, RemoveEndpoint } from "../models/service";

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
            dto = new RemoveEndpoint(this.props.serviceName, this.props.endpoint.url),
            onComplete = () => {
                state.isLoading = false;
                me.setState(state);
            }

        state.isLoading = true;
        this.setState(state);
        
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