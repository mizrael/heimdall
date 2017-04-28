import * as React from "react";
import { Button } from "react-bootstrap";
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

    private onRemove() {
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
            <Button bsStyle="danger" onClick={() => this.onRemove()}>Remove</Button>;

        return <tr>
            <td>{this.props.endpoint.url}</td>
            <td>{actions}</td>
        </tr>;
    }
}