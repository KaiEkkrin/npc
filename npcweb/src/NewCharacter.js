import React from 'react';
import { Button, TextField, Typography } from '@material-ui/core';

export class NewCharacter extends React.Component {
    render() {
        return (
            <form>
                <Typography>New character</Typography>
                <TextField id="name" label="Name" />
                <TextField id="level" label="Level" />
                <Button onClick={() => this.props.onCreateNew()}>Create</Button>
            </form>
        );
    }
}