import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShoesBrandLogosComponent } from './shoes-brand-logos.component';

describe('ShoesBrandLogosComponent', () => {
  let component: ShoesBrandLogosComponent;
  let fixture: ComponentFixture<ShoesBrandLogosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShoesBrandLogosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShoesBrandLogosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
