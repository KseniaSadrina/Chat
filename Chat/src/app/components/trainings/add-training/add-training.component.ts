import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';
import { ScenariosService } from 'src/app/services/scenarios.service';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { Scenario } from 'src/app/models/Scenario';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Training } from 'src/app/models/Training';

@Component({
  selector: 'app-add-training',
  templateUrl: './add-training.component.html',
  styleUrls: ['./add-training.component.css']
})
export class AddTrainingComponent implements OnInit, OnDestroy {

  constructor(public dialogRef: MatDialogRef<AddTrainingComponent>,
              private trainingsService: TrainingsService,
              private scenarioService: ScenariosService) { }

  public newTrainingForm = new FormGroup({
    name: new FormControl('', Validators.required),
    scenario: new FormControl(null, Validators.required),
    state: new FormControl(0)
  });

  scenarioDescription = new BehaviorSubject<string>('');
  scenarios: Observable<Scenario[]>;
  subscription: Subscription;

  compareFn: ((f1: Scenario, f2: Scenario) => boolean) | null = this.compareByValue;

  ngOnInit() {
    this.scenarios = this.scenarioService.scenarios$;
    if (this.newTrainingForm.controls.scenario) {
      this.subscription = this.newTrainingForm.controls.scenario.valueChanges.subscribe((res: Scenario) => {
        this.scenarioDescription.next(res.description);
      });
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  compareByValue(f1: any, f2: any) {
    return f1 && f2 && f1.id === f2.id;
  }

  onSubmit() {
    const trainingObj = new Training(this.newTrainingForm.value);
    this.trainingsService.startNewTraining(trainingObj);
    this.dialogRef.close();
  }

}
