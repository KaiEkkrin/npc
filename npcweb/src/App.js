import React from 'react';
import { AppBar, Toolbar, IconButton, Typography, Container } from '@material-ui/core';
import MenuIcon from '@material-ui/icons/Menu';
import './App.css';
import { Character } from './Character';
import { NewCharacter } from './NewCharacter';
import { NpcService } from './NpcService';

class Summary extends React.Component {
  render() {
    return (
      <Container maxWidth="lg">
        <NewCharacter onCreateNew={() => this.props.onCreateNew()} />
      </Container>
    );
  }
}

class Detail extends React.Component {
  render() {
    return (
      <Container maxWidth="lg">
        <Character data={this.props.data} />
      </Container>
    )
  }
}

export class App extends React.Component {
  constructor(props) {
    super(props);
    this.npcService = new NpcService('https://localhost:5001');
    this.state = { characterData: undefined };
  }

  getContent() {
    if (this.state.characterData) {
      return <Detail data={this.state.characterData} />;
    } else {
      return <Summary onCreateNew={() => this.handleCreateNew()} />;
    }
  }

  handleCreateNew() {
    // TODO parameterise with the name and level!
    this.npcService.createCharacter()
      .then(j => this.setState({ characterData: j }));
  }

  handleMenuClick() {
    // For now, that'll navigate us back to the top level list
    this.setState({ characterData: undefined });
  }

  render() {
    return (
      <div>
        <AppBar position="static">
          <Toolbar>
            <IconButton edge="start" color="inherit" aria-label="menu"
                        onClick={() => this.handleMenuClick()} >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6">npc</Typography>
          </Toolbar>
        </AppBar>
        {this.getContent()}
      </div>
    );
  }
}
