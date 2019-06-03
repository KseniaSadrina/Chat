import { Component, OnInit } from '@angular/core';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, combineLatest } from 'rxjs';
import { Progress } from 'src/app/models/progress';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-mock-training',
  templateUrl: './mock-training.component.html',
  styleUrls: ['./mock-training.component.css']
})

export class MockTrainingComponent implements OnInit {

  public currentProgress: Observable<Progress>;

  constructor(private trainingService: TrainingsService) { }

  public ngOnInit() {
    this.currentProgress = combineLatest (
      this.trainingService.currentTraining$,
      this.trainingService.trainingsProgress$).pipe(
        map(combined => {
          const training = combined[0];
          const mockedTrainings = combined[1];
          if (!(training && mockedTrainings)) {
            return null;
          }
          if (!mockedTrainings[training.id]) {
            this.trainingService.generateMock(training.id);
            return null;
          }
          console.log(mockedTrainings[training.id]);
          return mockedTrainings[training.id] as Progress;
        })
      );

    this.currentProgress.subscribe( change => {
      console.log(change);
      console.log('change');
    });
  }

  public async startTraining() {
    await this.trainingService.startTrainingMock();
  }
}
