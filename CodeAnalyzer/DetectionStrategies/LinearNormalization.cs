namespace CodeAnalyzer.DetectionStrategies
{
    using System;

    internal class LinearNormalization
    {
        private double _measurementRangeMin;
        private double _measurementRangeMax;
        private double _desiredRangeMin;
        private double _desiredRangeMax;

        public static LinearNormalization WithMeasurementAndDesiredRange(int measurementMin, int measurementMax, int desiredMin, int desiredMax)
        {
            var linearNormalization = new LinearNormalization
                                          {
                                              _measurementRangeMin = measurementMin,
                                              _measurementRangeMax = measurementMax,
                                              _desiredRangeMin = desiredMin,
                                              _desiredRangeMax = desiredMax
                                          };
            return linearNormalization;
        }

        public double ValueFor(double measurement)
        {
            var maxedMeasurement = Math.Min(measurement, _measurementRangeMax);
            return (maxedMeasurement - _measurementRangeMin) / (_measurementRangeMax - _measurementRangeMin)
                   * (_desiredRangeMax - _desiredRangeMin) + _desiredRangeMin;
        }
    }
}