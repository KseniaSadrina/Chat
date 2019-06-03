import { Scenario } from './Scenario';
import { TrainingState } from './enums/training-state';

export class Training {

    id: number;
    name: string;
    scenario: Scenario;
    state: TrainingState;

    public constructor(init?: Partial<Training >) {
        Object.assign(this, init);
    }
}
