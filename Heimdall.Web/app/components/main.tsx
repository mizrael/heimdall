﻿import * as React from "react";
import * as $ from "jquery";

export interface MainProps { }
export interface MainState {  }

export class Main extends React.Component<MainProps, MainState> {
    public render() {
        return <div className="test">it works!</div>;
    }
}