import * as React from "react";
import * as $ from "jquery";

export class Loading extends React.Component<{}, {}> {
    constructor(props: any) {
        super(props);
    }

    render() {
        return <div className="progress">
            <div className="progress-bar progress-bar-info progress-bar-striped" role="progressbar"
                aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style={{ width: '100%' }}>
                <span className="sr-only">Loading...</span>
            </div>
        </div>;
    }
}