import { TrainingState } from './enums/TrainingState';
import { Scenario } from './Scenario';

export class Training {

    id: number;
    name: string;
    scenario: Scenario;
    state: TrainingState;

    public constructor(init?: Partial<Training >) {
        Object.assign(this, init);
    }
}