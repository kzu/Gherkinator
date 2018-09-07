Feature: Sample
  A sample feature showing how to run the file from xunit

  Background:
    Given Foo
"""
<Application />
"""

  Scenario: Foo should equal Bar
    Given Bar 
"""
<Application />
"""
    When running test
    And doing something
    Then foo equals bar
    And succeeds

  Scenario: Steps can set state
    Given a saved value 10
    When a value 20 is also saved
    Then can add two values from state
    And verify the result