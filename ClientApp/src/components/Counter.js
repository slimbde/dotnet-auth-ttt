import React, { Component } from 'react';

export class Counter extends Component {
    static displayName = Counter.name;

    get = () => {
        fetch(`api/user`)
            .then(response => {
                if (response.status > 200)
                    alert("Вам сюда нельзя")
                else return response.json()
            })
            .then(console.log)
            .catch(console.error)
    }


    insert = () => {
        fetch(`api/user/InsertTest?name=Igor&age=55`)
            .then(console.log)
            .catch(console.error)
    }


    render() {
        return (
            <div>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <button className="btn btn-primary" onClick={this.get}>Increment</button>
                <button className="btn btn-primary" onClick={this.insert}>Increment</button>
            </div>
        );
    }
}
