import React, { Component } from 'react';

export class Brewcrafters extends Component {
    displayName = Brewcrafters.name

  constructor(props) {
    super(props);
      this.state = { loading: false};
      this.generateBrewcrafters = this.generateBrewcrafters.bind(this);
  }

    generateBrewcrafters() {
        this.setState({ loading: true });
        fetch('api/PdfGenerator/GenerateBrewcrafters')
            .then(data => {
                this.setState({ loading: false });
            });
    }
 
    render() {
    let contents = this.state.loading ?
        <p><em>Creating...</em></p> :
        <div></div>;
    return (
      <div>
        <h1>Brewcrafters</h1>
        <form method='post' action='api/PdfGenerator/GenerateBrewcrafters'>
                <input type='submit' className='btn btn-default' value='Generate Labels'></input>
            {contents}
        </form>
      </div>
    );
  }
}
