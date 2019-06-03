import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MockTrainingComponent } from './mock-training.component';

describe('MockTrainingComponent', () => {
  let component: MockTrainingComponent;
  let fixture: ComponentFixture<MockTrainingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MockTrainingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MockTrainingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
